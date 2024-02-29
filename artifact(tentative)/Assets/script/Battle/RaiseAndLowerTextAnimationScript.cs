using System.Collections;
using UnityEngine;
using TMPro;

public class RaiseAndLowerTextAnimationScript : MonoBehaviour
{
    //目的のYの値
    [SerializeField]
    private float destinationYValue = 0.5f;
    //文字の移動スピード
    [SerializeField]
    private float charSpeed = 5f;

    private TMP_Text m_TextComponent;
    private bool hasTextChanged;

    private VertexAnim[] vertexAnims;
    private TMP_TextInfo textInfo;
    private CharacterAnim[] characterAnims;
    private TMP_MeshInfo[] cachedMeshInfo;
    private TMP_CharacterInfo charInfo;

    //頂点用構造体
    private struct VertexAnim
    {
        public float yValue;
    }
    //文字毎の構造体
    private struct CharacterAnim 
    {
        public bool isAnimationStart;
        public bool isAnimationEnd;
        public bool isFlap;
    }

    private void Awake()
    {
        m_TextComponent = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
    }

    private void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
    }
    // Start is called before the first frame update
    void Start()
    {
        //強制メッシュアップデート、ないとエラーになる
        m_TextComponent.ForceMeshUpdate();

        textInfo = m_TextComponent.textInfo;

        hasTextChanged = true;

        //文字に使っている頂点用データの作成(×4は4隅分)
        vertexAnims = new VertexAnim[textInfo.characterCount * 4];
        //文字毎のデータ作成
        characterAnims = new CharacterAnim[textInfo.characterCount];
        //最初の文字のアニメーションをスタートさせる
        characterAnims[0].isAnimationStart = true;
        //コピーしておいた頂点データを格納
        cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

        //テキストを変更時、新たな頂点をコピーする
        if (hasTextChanged)
        {
            //次のテキストオブジェクトにテキストをコピーする
            cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

            hasTextChanged = false;
        }
    }

    void ON_TEXT_CHANGED(Object obj)
    {
        if (obj == m_TextComponent)
        {
            hasTextChanged = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        m_TextComponent.ForceMeshUpdate();

        Matrix4x4 matrix;

        int characterCount = textInfo.characterCount;

        if (characterCount == 0)
        {
            return;
        }

        //使用する変数宣言
        Vector3[] charOffset = new Vector3[characterCount];
        int materialIndex;
        int vertexIndex;
        Vector3[] sourceVertices;
        Vector2 charMidBasline;
        Vector3 offset;
        Vector3[] destinationVertices;

        for(int i=0; i<characterCount; i++)
        {
            //その文字のアニメーションがスタートしてなければ次の文字
            if (!characterAnims[i].isAnimationStart)
            {
                continue;
            }
            //その文字のアニメーションが終わっていれば次の文字
            if (characterAnims[i].isAnimationEnd)
            {
                continue;
            }

            charInfo = textInfo.characterInfo[i];

            //文字が見えなければ(空白文字)次の文字をアニメーションさせる
            if (!charInfo.isVisible)
            {
                characterAnims[i].isAnimationStart = false;
                characterAnims[i].isAnimationEnd = true;
                //最後の文字まで進んでいなければ次の文字をスタートさせる
                if (i < characterCount - 1)
                {
                    characterAnims[i + 1].isAnimationStart = true;
                }
                else
                {
                    //最後の文字が終わっていればゲームオブジェクトを1秒後に削除
                    Destroy(this.gameObject, 1f);
                }

                continue;
            }

            // Get the index of the material used by the current character.
            materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

            // Get the index of the first vertex used by this text element.
            vertexIndex = textInfo.characterInfo[i].vertexIndex;

            // Get the cached vertices of the mesh used by this text element (character or sprite).
            sourceVertices = cachedMeshInfo[materialIndex].vertices;

            // Determine the center point of each character at the baseline.
            //charMidBasline = new Vector2((sourceVertices[vertexIndex + 0].x + sourceVertices[vertexIndex + 2].x) / 2, charInfo.baseLine);
            // Determine the center point of each character.
            charMidBasline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

            // Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
            // This is needed so the matrix TRS is applied at the origin for each character.
            offset = charMidBasline;

            destinationVertices = textInfo.meshInfo[materialIndex].vertices;

            destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
            destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
            destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
            destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

            //　文字が上まで行っていない時は上に移動
            if (!characterAnims[i].isFlap)
            {
                charOffset[i] = new Vector3(0f, Mathf.MoveTowards(vertexAnims[i].yValue, destinationYValue, charSpeed * Time.deltaTime), 0f);
                vertexAnims[i].yValue = charOffset[i].y;
                Debug.Log(vertexAnims[i].yValue);
                //　目的地を越えたら反転
                if (charOffset[i].y >= destinationYValue)
                {
                    characterAnims[i].isFlap = true;
                }
            }
            else
            {
                charOffset[i] = new Vector3(0f, Mathf.MoveTowards(vertexAnims[i].yValue, 0f, charSpeed * Time.deltaTime), 0f);
                vertexAnims[i].yValue = charOffset[i].y;
                //　文字が元の位置に戻ったら次の文字をスタート
                if (vertexAnims[i].yValue <= 0f)
                {
                    characterAnims[i].isAnimationStart = false;
                    characterAnims[i].isAnimationEnd = true;
                    //　最後の文字でなければ次の文字をスタート
                    if (i < characterCount - 1)
                    {
                        characterAnims[i + 1].isAnimationStart = true;
                    }
                    else
                    {
                        //　最後の文字であればゲームオブジェクトを1秒後に削除
                        Destroy(this.gameObject, 1f);
                    }
                }
            }
            //　位置、回転、スケールから行列を作成
            matrix = Matrix4x4.TRS(charOffset[i], Quaternion.identity, Vector3.one);
            //　引数で与えた頂点をmatrix変換行列に従って変換
            destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
            destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
            destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
            destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

            destinationVertices[vertexIndex + 0] += offset;
            destinationVertices[vertexIndex + 1] += offset;
            destinationVertices[vertexIndex + 2] += offset;
            destinationVertices[vertexIndex + 3] += offset;

        }

        // Push changes into meshes
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            m_TextComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}
